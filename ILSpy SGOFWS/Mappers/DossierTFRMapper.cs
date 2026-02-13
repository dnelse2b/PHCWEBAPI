using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.Configuration;
using SGOFWS.Domains.Models;
using SGOFWS.DTOs;
using SGOFWS.Extensions;
using SGOFWS.Helper;

namespace SGOFWS.Mappers;

public class DossierTFRMapper
{
	private readonly ConversionExtension conversionExtension = new ConversionExtension();

	private readonly ConfiguracaoHelper configuracaoHelper = new ConfiguracaoHelper();

	public Dossier mapDossier(ConsignacaoTFRDTO consignacaoTFRDTO)
	{
		MapperConfiguration config = new MapperConfiguration(delegate(IMapperConfigurationExpression cfg)
		{
			cfg.CreateMap<ConsignacaoTFRDTO, Dossier>().ForPath((Dossier dest) => dest.bo2.Bo2stamp, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
			{
				act.MapFrom((ConsignacaoTFRDTO src) => "stamp");
			}).ForPath((Dossier dest) => dest.bo3.UConsgno, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
			{
				act.MapFrom((ConsignacaoTFRDTO src) => conversionExtension.nullToString(src.consignment_no));
			})
				.ForPath((Dossier dest) => dest.bo3.UConsgtip, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => conversionExtension.nullToString(src.consignment_type));
				})
				.ForPath((Dossier dest) => dest.bo.No, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, decimal?> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => 0);
				})
				.ForPath((Dossier dest) => dest.bo.Nome, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => "");
				})
				.ForPath((Dossier dest) => dest.bo.Ousrdata, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, DateTime?> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => DateTime.Now);
				})
				.ForPath((Dossier dest) => dest.bo.Usrdata, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, DateTime?> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => DateTime.Now);
				})
				.ForPath((Dossier dest) => dest.bo.Usrhora, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => DateTime.Now.ToString("HH:mm"));
				})
				.ForPath((Dossier dest) => dest.bo.Usrhora, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => DateTime.Now.ToString("HH:mm"));
				})
				.ForPath((Dossier dest) => dest.bo2.Ousrdata, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, DateTime?> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => DateTime.Now);
				})
				.ForPath((Dossier dest) => dest.bo2.Usrdata, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, DateTime?> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => DateTime.Now);
				})
				.ForPath((Dossier dest) => dest.bo2.Usrhora, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => DateTime.Now.ToString("HH:mm"));
				})
				.ForPath((Dossier dest) => dest.bo2.Usrhora, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => DateTime.Now.ToString("HH:mm"));
				})
				.ForPath((Dossier dest) => dest.bo3.Ousrdata, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, DateTime?> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => DateTime.Now);
				})
				.ForPath((Dossier dest) => dest.bo3.Usrdata, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, DateTime?> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => DateTime.Now);
				})
				.ForPath((Dossier dest) => dest.bo3.Usrhora, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => DateTime.Now.ToString("HH:mm"));
				})
				.ForPath((Dossier dest) => dest.bo3.Usrhora, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => DateTime.Now.ToString("HH:mm"));
				})
				.ForPath((Dossier dest) => dest.bo3.UNocl, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, decimal?> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => conversionExtension.toDecimal(conversionExtension.nullToString(src.customer_acc_no)));
				})
				.ForPath((Dossier dest) => dest.bo3.UCliente, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => conversionExtension.nullToString(src.customer_name));
				})
				.ForPath((Dossier dest) => dest.bo3.UStcarrg, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => conversionExtension.nullToString(src.load_station_name));
				})
				.ForPath((Dossier dest) => dest.bo3.UCodstcag, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => conversionExtension.nullToString(src.load_station_code));
				})
				.ForPath((Dossier dest) => dest.bo3.UDesvcarg, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => conversionExtension.nullToString(src.load_sdg_name));
				})
				.ForPath((Dossier dest) => dest.bo3.UCoddesvg, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => conversionExtension.nullToString(src.load_sdg_code));
				})
				.ForPath((Dossier dest) => dest.bo3.UStdest, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => conversionExtension.nullToString(src.dest_station_name));
				})
				.ForPath((Dossier dest) => dest.bo3.UCodstdet, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => conversionExtension.nullToString(src.dest_station_code));
				})
				.ForPath((Dossier dest) => dest.bo3.UCoddesvt, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => conversionExtension.nullToString(src.dest_sdg_code));
				})
				.ForPath((Dossier dest) => dest.bo3.UDesvdest, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => conversionExtension.nullToString(src.dest_sdg_name));
				})
				.ForPath((Dossier dest) => dest.bo3.UExped, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => conversionExtension.nullToString(src.consignor_name));
				})
				.ForPath((Dossier dest) => dest.bo3.UExpedno, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => conversionExtension.nullToString(src.consignor_no));
				})
				.ForPath((Dossier dest) => dest.bo3.UConsgt, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => conversionExtension.nullToString(src.consignee_name));
				})
				.ForPath((Dossier dest) => dest.bo3.UConsgtno, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => conversionExtension.nullToString(src.consignee_no));
				})
				.ForPath((Dossier dest) => dest.bo3.UStkdno, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => conversionExtension.nullToString(src.stakeholder_no));
				})
				.ForPath((Dossier dest) => dest.bo3.UStkdnome, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, string> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => conversionExtension.nullToString(src.stakeholder_name));
				})
				.ForPath((Dossier dest) => dest.bo3.UTotveicl, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, decimal?> act)
				{
					act.MapFrom((ConsignacaoTFRDTO src) => conversionExtension.toDecimal(conversionExtension.nullToString(src.wagon_count)));
				})
				.ForPath((Dossier dest) => dest.bi, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, List<Bi>> o)
				{
					o.MapFrom((ConsignacaoTFRDTO src) => src.wagons.Select((WagonTFRDTO wg) => new Bi
					{
						Nome = conversionExtension.nullToString(src.customer_name),
						No = conversionExtension.toDecimal(conversionExtension.nullToString(src.customer_acc_no)),
						Ousrdata = DateTime.Now,
						Usrdata = DateTime.Now,
						Usrhora = DateTime.Now.ToString("HH:mm"),
						Ousrhora = DateTime.Now.ToString("HH:mm")
					}));
				})
				.ForPath((Dossier dest) => dest.bi2, delegate(IPathConfigurationExpression<ConsignacaoTFRDTO, Dossier, List<Bi2>> o)
				{
					o.MapFrom((ConsignacaoTFRDTO src) => src.wagons.Select((WagonTFRDTO wg) => new Bi2
					{
						UDatcarrg = wg.load_date.ToString(),
						UHorcarrg = conversionExtension.nullToString(wg.load_time),
						UVagno = conversionExtension.nullToString(wg.wagon_no),
						UAdmintr = conversionExtension.nullToString(wg.wagon_owner),
						UVagtip = conversionExtension.nullToString(wg.wagon_type),
						UVagdesc = conversionExtension.nullToString(wg.wagon_desc),
						UVagcod = conversionExtension.nullToString(wg.wagon_code),
						UDualcon = conversionExtension.toBool(conversionExtension.nullToString(wg.dual_contract)),
						UContdcod = conversionExtension.nullToString(wg.contents_code),
						UCntdesc = conversionExtension.nullToString(wg.contents_desc),
						UDsccmcag = conversionExtension.nullToString(wg.commercial_comdty_desc),
						UPeso = conversionExtension.toTons(conversionExtension.nullToString(wg.wagon_mass)),
						UVagvol = conversionExtension.toDecimal(conversionExtension.nullToString(wg.wagon_vol)),
						UEncerrno = conversionExtension.nullToString(wg.tarpaulin_no),
						UEncrradm = conversionExtension.nullToString(wg.tarpaulin_adm),
						UComboio = conversionExtension.nullToString(wg.train_no),
						UProxst = "Komatiport",
						UCodactst = conversionExtension.nullToString(wg.current_station_code),
						UStact = conversionExtension.nullToString(wg.current_station_name),
						UFullempy = conversionExtension.toBool(conversionExtension.nullToString(wg.load_empty_ind)),
						UUltdtrep = wg.last_reported_date_time.ToString(),
						UStatus = conversionExtension.nullToString(wg.enroute_status_to_ktr),
						UEta = conversionExtension.nullToString(wg.eta_to_ktr),
						UEtafrt = conversionExtension.nullToString(wg.eta_to_ktr),
						UTotcont = conversionExtension.toDecimal(conversionExtension.nullToString(wg.container_count)),
						UUltmtmst = wg.last_update_timestamp.ToString(),
						Usrhora = DateTime.Now.ToString("HH:mm"),
						Usrdata = DateTime.Now,
						Ousrdata = DateTime.Now,
						Ousrhora = DateTime.Now.ToString("HH:mm"),
						contentores = wg.containers.Select((ContainerTFRDTO cnt) => new UBocont
						{
							Nocl = conversionExtension.nullToString(cnt.customer_acc_no),
							customerName = conversionExtension.nullToString(cnt.customer_name),
							Dtcarrg = cnt.load_date.ToString(),
							Hrcarrg = conversionExtension.nullToString(cnt.load_time),
							Stcarrg = conversionExtension.nullToString(cnt.load_station_name),
							Desvcarrg = conversionExtension.nullToString(cnt.load_sdg_name),
							Codstcarrg = conversionExtension.nullToString(cnt.load_station_code),
							Coddesvcarrg = conversionExtension.nullToString(cnt.load_sdg_code),
							Stdest = conversionExtension.nullToString(cnt.dest_station_name),
							Desvdest = conversionExtension.nullToString(cnt.dest_sdg_name),
							Codstdest = conversionExtension.nullToString(cnt.dest_station_code),
							Coddesvdest = conversionExtension.nullToString(cnt.dest_sdg_code),
							Expedidorno = conversionExtension.nullToString(cnt.consignor_no),
							Expedidor = conversionExtension.nullToString(cnt.consignor_name),
							Consgt = conversionExtension.nullToString(cnt.consignee_name),
							Consgtno = conversionExtension.nullToString(cnt.consignee_no),
							Vagno = conversionExtension.nullToString(wg.wagon_no),
							Nocontent = conversionExtension.nullToString(cnt.container_no),
							Dsccmcarg = conversionExtension.nullToString(cnt.commercial_comdty_desc),
							Codcarg = conversionExtension.nullToString(cnt.commdty_code),
							Contordno = conversionExtension.nullToString(cnt.sap_order_no),
							Peso = conversionExtension.toTons(conversionExtension.nullToString(cnt.container_mass)),
							Ousrdata = DateTime.Now,
							Usrdata = DateTime.Now,
							Usrhora = DateTime.Now.ToString("HH:mm"),
							Ousrhora = DateTime.Now.ToString("HH:mm")
						}).ToList()
					}));
				});
		});
		Mapper mapper = new Mapper(config);
		return mapper.Map<Dossier>(consignacaoTFRDTO);
	}
}
